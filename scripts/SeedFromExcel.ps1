param(
    [Parameter(Mandatory = $true)]
    [string]$ExcelPath,
    [Parameter(Mandatory = $true)]
    [string]$OutDir
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

if (-not (Test-Path $ExcelPath)) {
    throw "Excel file not found: $ExcelPath"
}

if (-not (Test-Path $OutDir)) {
    New-Item -ItemType Directory -Path $OutDir | Out-Null
}

function Get-UsedRangeValues($ws) {
    $used = $ws.UsedRange
    return ,$used.Value2
}

$excel = New-Object -ComObject Excel.Application
$excel.Visible = $false
$wb = $excel.Workbooks.Open($ExcelPath)

function Read-ColumnValues($values, $colIndex, $startRow) {
    $valuesOut = @()
    $rows = $values.GetLength(0)
    for ($r = $startRow; $r -le $rows; $r++) {
        $val = $values[$r, $colIndex]
        if ($null -ne $val -and -not [string]::IsNullOrWhiteSpace([string]$val)) {
            $valuesOut += ([string]$val).Trim()
        }
    }
    return $valuesOut
}

function Write-Json($path, $obj) {
    $json = $obj | ConvertTo-Json -Depth 5
    [System.IO.File]::WriteAllText($path, $json)
}

# Clients
$clientsWs = $wb.Sheets.Item("Clients")
$clientsValues = Get-UsedRangeValues $clientsWs
$clients = Read-ColumnValues $clientsValues 1 1 | Sort-Object -Unique | ForEach-Object { @{ Name = $_ } }
Write-Json (Join-Path $OutDir "clients.json") $clients

# Bulls
$bullsWs = $wb.Sheets.Item("Bulls")
$bullsValues = Get-UsedRangeValues $bullsWs
$bulls = @()
$rows = $bullsValues.GetLength(0)
for ($r = 1; $r -le $rows; $r++) {
    $name = $bullsValues[$r, 1]
    if ($null -ne $name -and -not [string]::IsNullOrWhiteSpace([string]$name)) {
        $nameText = ([string]$name).Trim()
        $code = $bullsValues[$r, 2]
        $codeText = if ($null -ne $code) { ([string]$code).Trim() } else { $null }
        $bulls += @{ Name = $nameText; Code = $(if ($codeText) { $codeText } else { $null }) }
    }
}
$bulls = $bulls | Sort-Object Name, Code -Unique
Write-Json (Join-Path $OutDir "bulls.json") $bulls

# Donors
$donorsWs = $wb.Sheets.Item("Donors")
$donorsValues = Get-UsedRangeValues $donorsWs
$donors = @()
$rows = $donorsValues.GetLength(0)
for ($r = 1; $r -le $rows; $r++) {
    $code = $donorsValues[$r, 1]
    if ($null -ne $code -and -not [string]::IsNullOrWhiteSpace([string]$code)) {
        $codeText = ([string]$code).Trim()
        $owner = $donorsValues[$r, 2]
        $ownerText = if ($null -ne $owner) { ([string]$owner).Trim() } else { $null }
        $program = $donorsValues[$r, 3]
        $programText = if ($null -ne $program) { ([string]$program).Trim() } else { $null }
        $embryo = $donorsValues[$r, 5]
        $embryoText = if ($null -ne $embryo) { ([string]$embryo).Trim() } else { $null }
        $embryoInt = $null
        if ($embryoText -match '^\d+$') {
            $embryoInt = [int]$embryoText
        }
        $donors += @{
            Code = $codeText
            OwnerName = $(if ($ownerText) { $ownerText } else { $null })
            ProgramName = $(if ($programText) { $programText } else { $null })
            EmbryoCount = $embryoInt
        }
    }
}
$donors = $donors | Sort-Object Code -Unique
Write-Json (Join-Path $OutDir "donors.json") $donors

# Programs from Donors + Implants
$programs = @()
if ($donors.Count -gt 0) {
    $programs += $donors | Where-Object { $_.ProgramName } | ForEach-Object { $_.ProgramName }
}

$implantsWs = $wb.Sheets.Item("Implants")
$implantsValues = Get-UsedRangeValues $implantsWs
$implantsHeaderCols = $implantsValues.GetLength(1)
$programCol = $null
$technicianCol = $null
for ($c = 1; $c -le $implantsHeaderCols; $c++) {
    $header = $implantsValues[1, $c]
    if ($null -ne $header) { $header = ([string]$header).Trim() } else { $header = "" }
    if ($header -eq "Program") { $programCol = $c }
    if ($header -eq "Technician") { $technicianCol = $c }
}

if ($programCol) {
    $programs += Read-ColumnValues $implantsValues $programCol 2
}

$programs = $programs | Where-Object { $_ } | Sort-Object -Unique | ForEach-Object { @{ Name = $_ } }
Write-Json (Join-Path $OutDir "programs.json") $programs

# Technicians from Implants
$technicians = @()
if ($technicianCol) {
    $technicians = Read-ColumnValues $implantsValues $technicianCol 2 | Sort-Object -Unique | ForEach-Object { @{ Name = $_ } }
}
Write-Json (Join-Path $OutDir "technicians.json") $technicians

try {
    $wb.Close($false)
    $excel.Quit()
} finally {
    [System.Runtime.Interopservices.Marshal]::ReleaseComObject($wb) | Out-Null
    [System.Runtime.Interopservices.Marshal]::ReleaseComObject($excel) | Out-Null
}

Write-Host "Seed files written to $OutDir"
