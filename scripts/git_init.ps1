$git = 'C:\Program Files\Git\cmd\git.exe'
& $git config user.name "$Env:USERNAME"
& $git config user.email "$Env:USERNAME@local"
& $git init
& $git branch -M main
& $git add .
& $git commit -m "Initial commit"
