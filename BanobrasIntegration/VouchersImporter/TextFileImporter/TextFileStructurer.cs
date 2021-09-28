/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                 Component : Vouchers Importer                    *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll   Pattern   : Structurer                           *
*  Type     : TextFileStructurer                            License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Provides information structure services for vouchers contained in text Files.                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Linq;

using Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter.Adapters;

namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter {

  /// <summary>Provides information structure services for vouchers contained in text Files.</summary>
  internal class TextFileStructurer {

    private readonly ImportVouchersCommand _command;
    private readonly string[] _textFileLines;

    private readonly FixedList<TextFileVoucherEntry> _entries;

    public TextFileStructurer(ImportVouchersCommand command, string[] textFileLines) {
      Assertion.AssertObject(command, "command");
      Assertion.AssertObject(textFileLines, "textFileLines");

      _command = command;
      _textFileLines = textFileLines;

      EnsureFileLinesAreValid();

      _entries = GetEntries();
    }


    public FixedList<TextFileVoucherEntry> Entries {
      get {
        return _entries;
      }
    }


    public int LinesCount {
      get {
        return _textFileLines.Length;
      }
    }

    #region Private methods

    private void EnsureFileLinesAreValid() {
      Assertion.Assert(this.LinesCount >= 2, "El archivo de texto contiene menos de dos registros.");
    }

    private FixedList<TextFileVoucherEntry> GetEntries() {
      var accountsChart = AccountsChart.Parse(_command.AccountsChartUID);

      return new FixedList<TextFileVoucherEntry>(_textFileLines.Select(x => new TextFileVoucherEntry(accountsChart.MasterData, x)));
    }

    #endregion Private methods

  }  // class TextFileStructurer

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter
