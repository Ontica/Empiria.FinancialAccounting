/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reconciliation Services                    Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Reconciliation.dll     Pattern   : Service provider                        *
*  Type     : OperationalEntriesReader                   License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Reads operational data entries from a dataset file for a reconciliation process.               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.Office;

using Empiria.FinancialAccounting.Datasets;

using Empiria.FinancialAccounting.Reconciliation.Adapters;


namespace Empiria.FinancialAccounting.Reconciliation {

  /// <summary>Reads operational data entries from a dataset file for a reconciliation process.</summary>
  internal class OperationalEntriesReader {

    private readonly Dataset _dataset;

    public OperationalEntriesReader(Dataset dataset) {
      Assertion.AssertObject(dataset, "dataset");

      _dataset = dataset;
    }


    internal bool AllEntriesAreValid() {
      try {
        this.GetEntries();
        return true;

      } catch {
        return false;
      }
    }

    internal FixedList<OperationalEntryDto> GetEntries() {
      Spreadsheet spreadsheet = OpenSpreadsheet();

      return ReadEntries(spreadsheet);
    }

    #region Private methods

    private Spreadsheet OpenSpreadsheet() {
      switch (_dataset.DatasetKind.FileType) {

        case FileType.Csv:
          return Spreadsheet.CreateFromCSVFile(_dataset.FullPath);

        case FileType.Excel:
          return Spreadsheet.Open(_dataset.FullPath);

        default:
          throw Assertion.AssertNoReachThisCode(
            $"Unhandled dataset file type '{_dataset.DatasetKind.FileType}'."
          );
      }
    }


    private FixedList<OperationalEntryDto> ReadEntries(Spreadsheet spreadsheet) {
      int row = 2;

      var entriesList = new List<OperationalEntryDto>(4096);

      while (spreadsheet.HasValue($"A{row}")) {
        OperationalEntryDto entry = ReadOperationalEntry(spreadsheet, row);

        entriesList.Add(entry);

        row++;
      }

      EmpiriaLog.Info(
        $"Se leyeron {entriesList.Count} registros del archivo de conciliación {_dataset.FullPath}."
      );

      return entriesList.ToFixedList();
    }


    private OperationalEntryDto ReadOperationalEntry(Spreadsheet spreadsheet, int row) {
      var helper = new ExcelRowReader(spreadsheet, row);

      return new OperationalEntryDto {
        UniqueKey = helper.GetUniqueKey(),
        LedgerNumber = helper.GetLedger(),
        AccountNumber = helper.GetAccountNumber(),
        SubledgerAccountNumber = helper.GetSubledgerAccountNumber(),
        CurrencyCode = helper.GetCurrencyCode(),
        SectorCode = helper.GetSectorCode(),
        TransactionSlip = helper.GetTransactionSlip(),
        ExtData = helper.GetExtensionData(),
        InitialBalance = helper.GetInitialBalance(),
        Debits = helper.GetDebits(),
        Credits = helper.GetCredits(),
        EndBalance = helper.GetEndBalance(),
        Position = row
      };
    }

    #endregion Private methods

  }  // class OperationalEntriesReader

} // namespace Empiria.FinancialAccounting.Reconciliation
