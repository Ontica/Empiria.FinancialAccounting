/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Accounting                       Component : Common Types                            *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Static methods library                  *
*  Type     : FileUtilities                              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Contains methods for file handling.                                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.IO;
using System.Text;

namespace Empiria.FinancialAccounting {

  /// <summary>Contains methods for file handling.</summary>
  static public class FileUtilities {

    static public string[] ReadTextFile(FileInfo textFile) {
      var ansiEncoding = Encoding.GetEncoding(1252);

      return File.ReadAllLines(textFile.FullName, ansiEncoding);
    }


    static public FileInfo SaveFile(FileData file) {
      string fileFullPath = ImportedFilePath(file);

      using (FileStream outputStream = File.OpenWrite(fileFullPath)) {
        file.InputStream.CopyTo(outputStream);
      }

      return new FileInfo(fileFullPath);
    }


    #region Private members

    static private Encoding GetFileEncoding(FileInfo textFile) {
      using (var reader = new StreamReader(textFile.FullName, true)) {
        while (reader.Peek() >= 0) {
          reader.Read();
        }

        return reader.CurrentEncoding;
      }
    }

    static private string ImportedFilesStoragePath {
      get {
        return ConfigurationData.Get<string>("ImportedFiles.StoragePath");
      }
    }

    static private string ImportedFilePath(FileData file) {
      var copyFileName = DateTime.Now.ToString("yyyy.MM.dd-HH.mm.ss-") + file.OriginalFileName;

      return Path.Combine(ImportedFilesStoragePath, copyFileName);
    }

    #endregion Private members

  }  // class FileUtilities

}  // namespace Empiria.FinancialAccounting
