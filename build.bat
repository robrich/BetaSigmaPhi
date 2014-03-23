msbuild BetaSigmaPhi.sln /t:Rebuild /p:Configuration=Release /verbosity:minimal
echo %ERRORLEVEL%