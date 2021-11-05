src=Program.cs Filters/*.cs Filters/JPEG/*.cs Filters/JPEG2000/*.cs Filters/JBIG2/*.cs Parser.cs Encryption.cs PDFFile.cs

/tmp/Program.exe:	$(src)
	dmcs $(src) -out:/tmp/Program.exe -r:System.Drawing.dll
