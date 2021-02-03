function Set-ProjectName {
    param ($projectName);

    $oldProjectName = (Get-ChildItem -Path "*.sln" -File).BaseName;
    Move-Item -Path ($oldProjectName + ".sln") -Destination ($projectName + ".sln");

    $projectItems = Get-ChildItem -Path "**\*.csproj" -File;
    foreach ($projectItem in $projectItems) {
        $oldName = $projectItem.Name;
        $newName = $oldName.Replace($oldProjectName, $projectName);
        Move-Item -Path $projectItem -Destination ($projectItem.DirectoryName + "\" + $newName);
    }

    $projectDirItems = Get-ChildItem -Path "*" -Directory;
    foreach ($projectDirItem in $projectDirItems) {
        $oldName = $projectDirItem.Name;
        $newName = $oldName.Replace($oldProjectName, $projectName);
        Move-Item -Path $oldName -Destination $newName;
    }

    $items = Get-ChildItem -Path "*" -Recurse -File;
    $secretRegex = [System.Text.RegularExpressions.Regex]::New("< *UserSecretsId *>([0-9a-zA-Z\-]+)< */ *UserSecretsId *>");
    foreach ($item in $items) {
        $content = Get-Content -Path $item;
        $content = $content.Replace($oldProjectName, $projectName);
        $match = $secretRegex.Match($content);
        if ($match.Success) {
            $content = $content.Replace($match.Groups[1], [System.Guid]::NewGuid().ToString());
        }
        $content.Replace($oldProjectName, $projectName) | Set-Content -Path $item;
    }
}

Export-ModuleMember -Function "Set-ProjectName";
