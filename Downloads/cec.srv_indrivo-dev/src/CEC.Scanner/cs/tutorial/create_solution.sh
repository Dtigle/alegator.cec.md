#!/bin/bash

echo Generating project files for MonoDevelop ...

for i in *.cs
do
  uids=$uids' '$(uuidgen)
done

function csproj
{
  m=0
  for uid in $uids
  do
    if [ "$m" -eq "$2" ]
    then
      projectguid=$uid
    fi
    ((m++))
  done
  echo -e -n '\xEF\xBB\xBF'
  echo '<?xml version="1.0" encoding="utf-8"?>'
  echo '<Project DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">'
  echo '  <PropertyGroup>'
  echo $'    <Configuration Condition=" \'$(Configuration)\' == \'\' ">Debug</Configuration>'
  echo $'    <Platform Condition=" \'$(Platform)\' == \'\' ">x86</Platform>'
  echo '    <ProductVersion>8.0.50727</ProductVersion>'
  echo '    <SchemaVersion>2.0</SchemaVersion>'
  echo '    <ProjectGuid>{'$projectguid'}</ProjectGuid>'
  echo '    <OutputType>Exe</OutputType>'
  echo '    <AppDesignerFolder>Properties</AppDesignerFolder>'
  echo '    <RootNamespace>'$1'</RootNamespace>'
  echo '    <AssemblyName>'$1'</AssemblyName>'
  echo '  </PropertyGroup>'
  echo $'  <PropertyGroup Condition=" \'$(Configuration)|$(Platform)\' == \'Debug|x86\' ">'
  echo '    <DebugSymbols>true</DebugSymbols>'
  echo '    <OutputPath>bin\x86\Debug\</OutputPath>'
  echo '    <DefineConstants>DEBUG;TRACE</DefineConstants>'
  echo '    <DebugType>full</DebugType>'
  echo '    <PlatformTarget>x86</PlatformTarget>'
  echo '    <ErrorReport>prompt</ErrorReport>'
  echo '    <WarningLevel>4</WarningLevel>'
  echo '    <Optimize>false</Optimize>'
  echo '  </PropertyGroup>'
  echo $'  <PropertyGroup Condition=" \'$(Configuration)|$(Platform)\' == \'Release|x86\' ">'
  echo '    <OutputPath>bin\x86\Release\</OutputPath>'
  echo '    <DefineConstants>TRACE</DefineConstants>'
  echo '    <Optimize>true</Optimize>'
  echo '    <DebugType>pdbonly</DebugType>'
  echo '    <PlatformTarget>x86</PlatformTarget>'
  echo '    <ErrorReport>prompt</ErrorReport>'
  echo '    <WarningLevel>4</WarningLevel>'
  echo '  </PropertyGroup>'
  echo '  <ItemGroup>'
  echo '    <Reference Include="Pr22" />'
  echo '    <Reference Include="System" />'
  echo '    <Reference Include="System.Data" />'
  echo '    <Reference Include="System.Drawing" />'
  echo '    <Reference Include="System.Xml" />'
  echo '  </ItemGroup>'
  echo '  <ItemGroup>'
  echo '  <Compile Include="..\..\'$1'.cs">'
  echo '  <Link>'$1'.cs</Link>'
  echo '  </Compile>'
  if [ "$1" -eq "pr05_doctype" ]
  then
    echo '  <Compile Include="..\..\Pr22.Extension\CountryCode.cs">'
    echo '  <Link>CountryCode.cs</Link>'
    echo '  </Compile>'
    echo '  <Compile Include="..\..\Pr22.Extension\DocumentType.cs">'
    echo '  <Link>DocumentType.cs</Link>'
    echo '  </Compile>'
  fi
  echo '    <Compile Include="AssemblyInfo.cs" />'
  echo '  </ItemGroup>'
  echo '  <Import Project="$(MSBuildBinPath)\Microsoft.CSharp.targets" />'
  echo '  <!-- To modify your build process, add your task inside one of the targets below and uncomment it. '
  echo '       Other similar extension points exist, see Microsoft.Common.targets.'
  echo '  <Target Name="BeforeBuild">'
  echo '  </Target>'
  echo '  <Target Name="AfterBuild">'
  echo '  </Target>'
  echo '  -->'
  echo '</Project>'
}

function AssemblyInfo
{
  echo 'using System.Reflection;'
  echo 'using System.Runtime.CompilerServices;'
  echo 'using System.Runtime.InteropServices;'
  echo '// General Information about an assembly is controlled through the following '
  echo '// set of attributes. Change these attribute values to modify the information'
  echo '// associated with an assembly.'
  echo '[assembly: AssemblyTitle("'$1'")]'
  echo '[assembly: AssemblyDescription("")]'
  echo '[assembly: AssemblyConfiguration("")]'
  echo '[assembly: AssemblyCompany("ARH Inc.")]'
  echo '[assembly: AssemblyProduct("'$1'")]'
  echo '[assembly: AssemblyCopyright("Copyright © ARH 2013")]'
  echo '[assembly: AssemblyTrademark("")]'
  echo '[assembly: AssemblyCulture("")]'
  echo '// Setting ComVisible to false makes the types in this assembly not visible '
  echo '// to COM components.  If you need to access a type in this assembly from '
  echo '// COM, set the ComVisible attribute to true on that type.'
  echo '[assembly: ComVisible(false)]'
  echo '// The following GUID is for the ID of the typelib if this project is exposed to COM'
  echo '[assembly: Guid("83aaf6fe-42c6-48f6-a296-126b3a94a4aa")]'
  echo '// Version information for an assembly consists of the following four values:'
  echo '//'
  echo '//      Major Version'
  echo '//      Minor Version '
  echo '//      Build Number'
  echo '//      Revision'
  echo '//'
  echo '[assembly: AssemblyVersion("2.2.0.0")]'
  echo '[assembly: AssemblyFileVersion("2.2.0.0")]'
}

function sln
{
  echo -e '\xEF\xBB\xBF'
  echo 'Microsoft Visual Studio Solution File, Format Version 9.00'
  echo '# Visual Studio 2005'

  m=0
  for i in *.cs
  do
    j=0
    for uid in $uids
    do
    if [ "$m" -eq "$j" ]
    then
      projectguid=$uid
      echo 'Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "'${i%.*}'", "tutorial\'${i%.*}'\'${i%.*}'.csproj", "{'$projectguid'}"'
      echo 'EndProject'
    fi
    ((j++))
    done
    ((m++))
  done

  echo 'Global'
  echo $'\tGlobalSection(SolutionConfigurationPlatforms) = preSolution'
  echo $'\t\tDebug|x86 = Debug|x86'
  echo $'\t\tRelease|x86 = Release|x86'
  echo $'\tEndGlobalSection'
  echo 'GlobalSection(ProjectConfigurationPlatforms) = postSolution'

  m=0
  for i in *.cs
  do
    j=0
    for uid in $uids
    do
    if [ "$m" -eq "$j" ]
    then
      projectguid=$uid
      echo $'\t{'$projectguid'}.Debug|x86.ActiveCfg = Debug|x86'
      echo $'\t{'$projectguid'}.Debug|x86.Build.0 = Debug|x86'
      echo $'\t{'$projectguid'}.Release|x86.ActiveCfg = Release|x86'
      echo $'\t{'$projectguid'}.Release|x86.Build.0 = Release|x86'
    fi
    done
    ((m++))
  done

  m=0
  for i in *.cs
  do
    if [ "$m" -eq "0" ]
    then
      echo $'\tEndGlobalSection'
      echo $'\tGlobalSection(MonoDevelopProperties) = preSolution'
      echo $'\t\tStartupItem = tutorial/'${i%.*}'/'${i%.*}'.csproj'
      echo $'\tEndGlobalSection'
      echo $'EndGlobal'
    fi
    ((m++))
  done


}


mkdir tutorial
counter=0
for i in *.cs
do
  mkdir tutorial/${i%.*}
  AssemblyInfo ${i%.*} $counter > tutorial/${i%.*}/AssemblyInfo.cs
  csproj ${i%.*} $counter > tutorial/${i%.*}/${i%.*}.csproj
  ((counter++))
done

sln > tutorial.sln

exit