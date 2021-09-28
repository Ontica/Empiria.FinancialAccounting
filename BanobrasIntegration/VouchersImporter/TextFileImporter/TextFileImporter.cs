/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                Component : Vouchers Importer                     *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll  Pattern   : Command Controller                    *
*  Type     : TextFileImporter                             License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Query web API used to retrive accounting vouchers.                                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.IO;

using Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter.Adapters;

namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter {

  internal class TextFileImporter {

    private readonly ImportVouchersCommand _command;
    private readonly FileInfo _textFile;

    internal TextFileImporter(ImportVouchersCommand command, FileInfo textFile) {
      Assertion.AssertObject(command, "command");
      Assertion.AssertObject(textFile, "textFile");

      _command = command;
      _textFile = textFile;
    }


    internal ImportVouchersResult DryRunImport() {
      string[] textFileLines = FileUtilities.ReadTextFile(_textFile);

      var contentParser = new TextFileStructurer(_command, textFileLines);

      return ImportVouchersResult.Default;
    }


    internal ImportVouchersResult Import() {
      string[] textFileLines = FileUtilities.ReadTextFile(_textFile);

      var contentParser = new TextFileStructurer(_command, textFileLines);

      return ImportVouchersResult.Default;
    }


  }  // class TextFileImporter

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter
