src=Filters/*.cs Filters/JPEG/*.cs Filters/JPEG2000/*.cs Filters/JBIG2/*.cs Parser.cs Encryption.cs PDFFile.cs Matrix.cs PdfGraphics.cs
gui_src=../MainForm/*.cs

all: /tmp/Program.exe /tmp/GUI.exe

/tmp/Program.exe:	$(src) Program.cs
	dmcs $^ -out:$@ -r:System.Drawing.dll

/tmp/GUI.exe: $(src) $(gui_src)
	dmcs $^ -out:$@ -r:System.Drawing.dll -r:System.Data.dll -r:System.Windows.Forms.dll
