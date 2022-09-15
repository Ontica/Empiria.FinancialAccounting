/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                            Component : Report Builders                      *
*  Assembly : FinancialAccounting.Reporting.dll             Pattern   : Report builder                       *
*  Type     : PolizaEntryDto                                License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Entry in 'Listado de Pólizas' report.                                                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.FinancialAccounting.Reporting.VoucherRelatedReports.Domain {

  /// <summary>Entry in 'Listado de Pólizas' report.</summary>
  public class PolizaReturnedEntry : IReportEntryDto {

    public string LedgerNumber {
      get; internal set;
    }


    public string LedgerName {
      get; internal set;
    } = string.Empty;


    public string VoucherNumber {
      get;
      internal set;
    } = string.Empty;


    public string AccountingDate {
      get; internal set;
    } = string.Empty;


    public string RecordingDate {
      get; internal set;
    } = string.Empty;


    public string ElaboratedBy {
      get; internal set;
    } = string.Empty;


    public string Concept {
      get; internal set;
    }


    public decimal Debit {
      get; internal set;
    }


    public decimal Credit {
      get; internal set;
    }

    public int VouchersByLedger {
      get; internal set;
    }


    public ItemType ItemType {
      get; internal set;
    } = ItemType.Entry;


  }  // class PolizaReturnedEntry

} // namespace Empiria.FinancialAccounting.Reporting.VoucherRelatedReports.Domain
