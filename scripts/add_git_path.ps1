$p = 'C:\Program Files\Git\cmd'
$user = [Environment]::GetEnvironmentVariable('PATH','User')
if ($user -notlike "*${p}*") {
  if ([string]::IsNullOrEmpty($user)) { $new = $p } else { $new = $user + ';' + $p }
  [Environment]::SetEnvironmentVariable('PATH', $new, 'User')
}
$env:PATH = [Environment]::GetEnvironmentVariable('PATH','User')
& (Join-Path $p 'git.exe') --version
