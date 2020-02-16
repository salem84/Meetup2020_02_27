Write-Host "Leggo build nella cartella K6"
$pipelines = az pipelines list --query "[?path=='\K6'].{Nome: name, Id: id}" | ConvertFrom-Json
$pipelines | 
    #Select-Object -first 1 | 
    foreach {
        Write-Host "Lancio Build [$($_.Id)] $($_.Nome)"
        az pipelines run --id $_.Id
    }