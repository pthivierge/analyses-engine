
:: Command line example

:: to write in PI, use the --EnableWrite option
:: to write in file, use the -f c:\Temp\datafile option.  a date and a csv extension will be added automatically.
::
AnalysesEngineCommandLine.exe -s optimus -d AF_MOS5 -e elements.txt --st 2014-08-15 --et 2015-07-14 --interval hourly --threadsCount 4 --AnalysesThreadsCount 8 -w 45

pause
