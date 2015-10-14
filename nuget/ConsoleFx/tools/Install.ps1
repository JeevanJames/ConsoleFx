param($installPath, $toolsPath, $package, $project)

$item = $project.ProjectItems.Item("Usage.txt")
$item.Properties.Item("BuildAction").Value = [int]3
