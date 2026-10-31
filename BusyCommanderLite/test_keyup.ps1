Add-Type -AssemblyName System.Windows.Forms
$form = New-Object System.Windows.Forms.Form
$form.KeyPreview = $true
$form.Add_KeyUp({
    param($sender, $e)
    if ($e.KeyCode -eq 'Escape') {
        Write-Host "KeyUp received ESC!"
        $form.Close()
    }
})
# We can't easily override ProcessCmdKey in powershell without a custom class,
# but we can trust .NET behavior: ProcessCmdKey only handles WM_KEYDOWN/SYSKEYDOWN.
$form.ShowDialog()
