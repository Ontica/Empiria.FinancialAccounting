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
using Empiria.Storage;

using Empiria.DynamicData.Datasets;

using Empiria.FinancialAccounting.Reconciliation.Adapters;
using Empiria.FinancialAccounting.Reconciliation.Readers;

namespace Empiria.FinancialAccounting.Reconciliation {

  /// <summary>Reads operational data entries from a dataset file for a reconciliation process.</summary>
  internal class OperationalEntriesReader {

    private readonly Dataset _dataset;

    public OperationalEntriesReader(Dataset dataset) {
      Assertion.Require(dataset, "dataset");

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
          throw Assertion.EnsureNoReachThisCode(
            $"Unhandled dataset file type '{_dataset.DatasetKind.FileType}'."
          );
      }
    }


    private FixedList<OperationalEntryDto> ReadEntries(Spreadsheet spreadsheet) {
      int rowIndex = 2;

      var entriesList = new List<OperationalEntryDto>(4096);

      while (spreadsheet.HasValue($"A{rowIndex}")) {
        OperationalEntryDto entry = ReadOperationalEntry(spreadsheet, rowIndex);

        entriesList.Add(entry);

        rowIndex++;
      }

      EmpiriaLog.Info(
        $"Se leyeron {entriesList.Count} registros del archivo de conciliación {_dataset.FullPath}."
      );

      return entriesList.ToFixedList();
    }


    private OperationalEntryDto ReadOperationalEntry(Spreadsheet spreadsheet, int rowIndex) {
      IReconciliationRowReader rowReader = GetRowReader(spreadsheet, rowIndex);

      return new OperationalEntryDto {
        UniqueKey               = rowReader.GetUniqueKey(),
        LedgerNumber            = rowReader.GetLedger(),
        AccountNumber           = rowReader.GetAccountNumber(),
        SubledgerAccountNumber  = rowReader.GetSubledgerAccountNumber(),
        CurrencyCode            = rowReader.GetCurrencyCode(),
        SectorCode              = rowReader.GetSectorCode(),
        TransactionSlip         = rowReader.GetTransactionSlip(),
        ExtData                 = rowReader.GetExtensionData(),
        InitialBalance          = rowReader.GetInitialBalance(),
        Debits                  = rowReader.GetDebits(),
        Credits                 = rowReader.GetCredits(),
        EndBalance              = rowReader.GetEndBalance(),
        Position                = rowIndex
      };
    }

    private IReconciliationRowReader GetRowReader(Spreadsheet spreadsheet, int rowIndex) {
      switch (_dataset.DatasetKind.DataFormat) {
        case "IkosDerivados":
          return new IkosDerivadosRowReader(spreadsheet, rowIndex);
        case "SimefinDerivados":
          return new SimefinRowReader(spreadsheet, rowIndex);
        default:
          throw Assertion.EnsureNoReachThisCode($"Unrecognized dataset kind format {_dataset.DatasetKind.DataFormat}.");
      }
    }

    #endregion Private methods

  }  // class OperationalEntriesReader

} // namespace Empiria.FinancialAccounting.Reconciliation
