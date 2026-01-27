internal class CompletionCommand : BaseCommand<CompletionSettings>
{
	protected override int ExecuteImpl(CompletionSettings settings)
	{
		if (settings.Shell.Equals("zsh", StringComparison.OrdinalIgnoreCase))
		{
			Console.WriteLine(GenerateZshCompletion());
			return 0;
		}

		WriteError($"Unsupported shell: {settings.Shell}. Currently only 'zsh' is supported.");
		return 1;
	}

	private static string GenerateZshCompletion()
	{
		return """
		       #compdef ucll

		       _ucll() {
		           local -a commands
		           commands=(
		               'open:Open Unity Editor for a project search path or via recent projects prompt'
		               'create:Create a new Unity project'
		               'project-path:Get Unity project root directory from search path or via recent projects prompt'
		               'editor-revision:Get revision for Unity version'
		               'editor-path:Get installation path for Unity version'
		               'editor-modules:List installed modules for Unity version'
		               'project-version:Get Unity version from project search directory or ProjectVersion.txt'
		               'version-usage:List installed Unity Editor versions and indicate which ones are used by projects'
		               'projects-using:Find all projects that use a specific Unity version'
		               'install:Install Unity Editor version'
		               'install-missing:Install all Unity versions that are used by projects but not currently installed'
		               'uninstall-unused:Uninstall all Unity versions that are not used by any projects'
		               'upm-git-url:Generate a git URL for Unity Package Manager from a Unity project'
		               'hub:Execute Unity Hub interactively or with additional CLI arguments'
		               'completion:Generate shell completion scripts'
		           )

		           _arguments -C \
		               '1: :->command' \
		               '*::arg:->args'

		           case "$state" in
		               command)
		                   _describe 'command' commands
		                   ;;
		               args)
		                   case $words[1] in
		                       open)
		                           _arguments \
		                               '(-f --favorite --favorites)'{-f,--favorite,--favorites}'[Use favorite projects only]' \
		                               '(-c --code-editor)'{-c,--code-editor}'[Open the solution file in the default code editor]' \
		                               '(-o --only-code-editor)'{-o,--only-code-editor}'[Open only the code editor without launching Unity]' \
		                               '--dry-run[Show what would be executed without actually running mutating commands]'
		                           ;;
		                       create)
		                           _arguments \
		                               '(-m --minimal)'{-m,--minimal}'[Creates a bare-minimum project (fast)]' \
		                               '--dry-run[Show what would be executed without actually running mutating commands]' \
		                               '1:project path:_directories' \
		                               '2:version:'
		                           ;;
		                       project-path)
		                           _arguments \
		                               '(-f --favorite --favorites)'{-f,--favorite,--favorites}'[Use favorite projects only]'
		                           ;;
		                       editor-revision)
		                           _arguments \
		                               '1:version:'
		                           ;;
		                       editor-path)
		                           _arguments \
		                               '1:version:'
		                           ;;
		                       editor-modules)
		                           _arguments \
		                               '1:version:'
		                           ;;
		                       project-version)
		                           _arguments \
		                               '(-f --favorite --favorites)'{-f,--favorite,--favorites}'[Use favorite projects only]'
		                           ;;
		                       version-usage)
		                           _arguments \
		                               '(-p --plaintext --plain)'{-p,--plaintext,--plain}'[Output in a simple machine-parseable format]' \
		                               '(-m --modules)'{-m,--modules}'[Include installed modules for each editor version]'
		                           ;;
		                       projects-using)
		                           _arguments \
		                               '1:version:'
		                           ;;
		                       install)
		                           _arguments \
		                               '--dry-run[Show what would be executed without actually running mutating commands]' \
		                               '1:version:' \
		                               '2:changeset:'
		                           ;;
		                       install-missing)
		                           _arguments \
		                               '--dry-run[Show what would be executed without actually running mutating commands]'
		                           ;;
		                       uninstall-unused)
		                           _arguments \
		                               '--dry-run[Show what would be executed without actually running mutating commands]'
		                           ;;
		                       upm-git-url)
		                           _arguments \
		                               '(-f --favorite --favorites)'{-f,--favorite,--favorites}'[Use favorite projects only]'
		                           ;;
		                       hub)
		                           # Hub command accepts arbitrary arguments, no specific completion
		                           ;;
		                       completion)
		                           _arguments \
		                               '1:shell:(zsh)'
		                           ;;
		                   esac
		                   ;;
		           esac
		       }

		       _ucll
		       """;
	}
}