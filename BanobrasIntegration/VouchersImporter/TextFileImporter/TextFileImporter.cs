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
      VoucherImporter voucherImporter = GetVoucherImporter();

      return voucherImporter.DryRunImport();
    }


    internal ImportVouchersResult Import() {
      VoucherImporter voucherImporter = GetVoucherImporter();

      return voucherImporter.Import();
    }


    private VoucherImporter GetVoucherImporter() {
      string[] textFileLines = FileUtilities.ReadTextFile(_textFile);

      var structurer = new TextFileStructurer(_command, textFileLines);

      FixedList<ToImportVoucher> toImport = structurer.GetToImportVouchersList();

      return new VoucherImporter(_command, toImport);
    }

  }  // class TextFileImporter

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter
