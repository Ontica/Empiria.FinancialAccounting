/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reconciliation Services                    Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Reconciliation.dll     Pattern   : Service provider                        *
*  Type     : ReconciliationDatasetEntriesReader         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Reads reconciliation data entries from a dataset file.                                         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.Office;

using Empiria.FinancialAccounting.Datasets;

using Empiria.FinancialAccounting.Reconciliation.Adapters;


namespace Empiria.FinancialAccounting.Reconciliation {

  /// <summary>Reads reconciliation data entries from a dataset file.</summary>
  internal class ReconciliationDatasetEntriesReader {

    private readonly Dataset _dataset;

    public ReconciliationDatasetEntriesReader(Dataset dataset) {
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

    internal FixedList<ReconciliationEntryDto> GetEntries() {
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


    private FixedList<ReconciliationEntryDto> ReadEntries(Spreadsheet spreadsheet) {
      int row = 2;

      var entriesList = new List<ReconciliationEntryDto>(4096);

      while (spreadsheet.HasValue($"A{row}")) {
        ReconciliationEntryDto entry = ReadReconciliationEntry(spreadsheet, row);

        entriesList.Add(entry);

        row++;
      }

      EmpiriaLog.Info(
        $"Se leyeron {entriesList.Count} registros del archivo de conciliación {_dataset.FullPath}."
      );

      return entriesList.ToFixedList();
    }


    private ReconciliationEntryDto ReadReconciliationEntry(Spreadsheet spreadsheet, int row) {
      var helper = new ExcelRowReader(spreadsheet, row);

      return new ReconciliationEntryDto {
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

  }  // class ReconciliationDatasetEntriesReader

} // namespace Empiria.FinancialAccounting.Reconciliation
