$p = 'C:\Program Files\GitHub CLI'
$user = [Environment]::GetEnvironmentVariable('PATH','User')
if ($user -notlike "*${p}*") {
  if ([string]::IsNullOrEmpty($user)) { $new = $p } else { $new = $user + ';' + $p }
  [Environment]::SetEnvironmentVariable('PATH', $new, 'User')
}
$env:PATH = [Environment]::GetEnvironmentVariable('PATH','User')
& (Join-Path $p 'gh.exe') --version
