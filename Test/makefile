src=TestRunner.cs ../PdfCS/Filters/*.cs ../PdfCS/Filters/JPEG/*.cs ../PdfCS/Filters/JBIG2/*.cs ../PdfCS/Filters/JPEG2000/*.cs ../PdfCS/Parser.cs *.cs ../PdfCS/Encryption.cs ../PdfCS/Matrix.cs ../PdfCS/PDFFile.cs

/tmp/Testing.exe:	$(src)
	dmcs $(src) -out:/tmp/Testing.exe -r:System.Drawing.dll
