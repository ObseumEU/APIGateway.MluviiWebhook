./ngrok/ngrok.exe authtoken 1qTchOu5iC7K0mxTrQL6pZZusF7_6uL44G6pLdqZ5Nki2wvFB

start powershell {./ngrok/ngrok.exe http http://localhost:5025 }

Write-Host "Wait for ngrok start!"
Start-Sleep -s 5