/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reconciliation Services                    Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Reconciliation.dll     Pattern   : Service provider                        *
*  Type     : ReconciliationEntriesImporter              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Reads and stores reconciliation data from dataset files.                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System.Collections.Generic;

using Empiria.Office;

using Empiria.FinancialAccounting.Datasets;

using Empiria.FinancialAccounting.Reconciliation.Adapters;

namespace Empiria.FinancialAccounting.Reconciliation {

  /// <summary>Reads and stores reconciliation data from dataset files.</summary>
  internal class ReconciliationEntriesImporter {

    private readonly Dataset _dataset;

    public ReconciliationEntriesImporter(Dataset dataset) {
      Assertion.AssertObject(dataset, "dataset");

      _dataset = dataset;
    }


    internal void Import() {
      Spreadsheet spreadsheet = OpenSpreadsheet();

      FixedList<ReconciliationEntry> entries = ReadEntries(spreadsheet);

      foreach (ReconciliationEntry entry in entries) {
        entry.Save();
      }
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


    private FixedList<ReconciliationEntry> ReadEntries(Spreadsheet spreadsheet) {
      int row = 2;

      var entriesList = new List<ReconciliationEntry>(4096);

      while (spreadsheet.HasValue($"A{row}")) {
        ReconciliationEntry entry = ReadReconciliationEntry(spreadsheet, row);

        entriesList.Add(entry);

        row++;
      }

      EmpiriaLog.Info(
        $"Se leyeron {entriesList.Count} registros del archivo de conciliación {_dataset.FullPath}."
      );

      return entriesList.ToFixedList();
    }


    private ReconciliationEntry ReadReconciliationEntry(Spreadsheet spreadsheet, int row) {
      var helper = new ExcelRowReader(spreadsheet, row);

      var dto = new ReconciliationEntryDto {
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

      return new ReconciliationEntry(_dataset, dto);
    }

    #endregion Private methods

  }  // class ReconciliationEntriesImporter

} // namespace Empiria.FinancialAccounting.Reconciliation
