require 'bundler/setup'
require 'albacore'

require_relative 'tools/semver.rb'

ci_build = ENV['APPVEYOR_BUILD_VERSION'] ||= "0"
ci_commit = ENV['APPVEYOR_REPO_COMMIT'] ||= "0"
ci_run = ENV['APPVEYOR'] || false

tool_nuget = 'tools/nuget/nuget.exe'
tool_xunit = 'tools/xunit/xunit.console.exe'

project_name = 'Overseer.Samples'
project_version = read_semver

project_output = 'build/bin'
package_output = 'build/deploy'

build_mode = ENV['mode'] ||= "Debug"

desc 'Restore nuget packages for all projects'
nugets_restore :restore do |n|
  n.exe = tool_nuget
  n.out = 'packages'

  n.parameters << %W[-configFile nuget.config]
end

desc 'Set the assembly version number'
asmver :version do |v|

  v.file_path = "AssemblyVersion.cs"
  v.attributes assembly_version: project_version,
               assembly_file_version: project_version,
               assembly_description: "Build: #{ci_build}, Commit Sha: #{ci_commit}"
end

desc 'Compile all projects'
build :compile do |msb|
  msb.target = [ :clean, :rebuild ]
  msb.prop 'configuration', build_mode
  msb.sln = "#{project_name}.sln"
end

desc 'Run all unit test assemblies'
test_runner :test do |xunit|

  files = FileList['**.Tests/bin/*/*.{acceptance,tests}.dll']
  files.exclude(/.*RabbitMQ.*/) if ci_run

  xunit.exe = tool_xunit
  xunit.files = files
  xunit.add_parameter '-quiet' if ci_run
  xunit.add_parameter '-nologo'
end

task :default => [ :restore, :version, :compile, :test ]
