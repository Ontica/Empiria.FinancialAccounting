/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                Component : Vouchers Importer                     *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll  Pattern   : Static methods library                *
*  Type     : FileUtilities                                License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Contains methods for file handling.                                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.IO;
using System.Text;

using Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter.Adapters;

namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter {

  static internal class FileUtilities {

    static internal string[] ReadTextFile(FileInfo textFile) {
      var ansiEncoding = Encoding.GetEncoding(1252);

      return File.ReadAllLines(textFile.FullName, ansiEncoding);
    }


    static internal FileInfo SaveFile(FileData file) {
      string fileFullPath = ImportedFilePath(file);

      //string content = file.InputStream.File.ReadAllText(
      //File.WriteAllBytes(fileFullPath, Encoding.UTF8.GetBytes(content);

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

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter
