$url = "https://meetup2020.azurewebsites.net/api/v1/foods/getrandommeal"

while ($true) { 
    $statusCode = 0
    $timeTaken = Measure-Command -Expression {
        try {
            $res = Invoke-WebRequest -Uri $url
            $statusCode = $res.StatusCode
        }
        catch { 
            $statusCode = $_.Exception.Response.StatusCode.Value__
        }
    }

    $milliseconds = [Math]::Round($timeTaken.TotalMilliseconds, 1)

    Write-Host "$(Get-Date -Format HH:mm:ss:ms) - Result: $statusCode [+$milliseconds]"
}