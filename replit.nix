{ pkgs }: {
	deps = [
		pkgs.mono
  pkgs.dotnet-sdk
    pkgs.omnisharp-roslyn
	];
}