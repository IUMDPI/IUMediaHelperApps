rmdir %2 /s /q
mkdir %2
copy %1\*.exe %2
copy %1\*.config %2
copy %1\*.dll %2
copy %1\*.doc %2
copy %1\*.docx %2
copy %1\*.txt %2
copy %1\*.bat %2
del %2\*.vshost.exe
del %2\*.vshost.exe.config
ren %2\*.exe.config *.config.dev
