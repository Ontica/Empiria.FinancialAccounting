/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration                          Component : Interface adapters                   *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll   Pattern   : Data Transfer Object                 *
*  Type     : OperationalReportDto                          License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Output DTO used to return operational reports.                                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.BanobrasIntegration.SATReports.Adapters {

  public interface IOperationalReportEntryDto {

  }

  /// <summary>Output DTO used to return operational reports.</summary>
  public class OperationalReportDto {

    public OperationalReportCommand Command {
      get; internal set;
    } = new OperationalReportCommand();


    public FixedList<DataTableColumn> Columns {
      get; internal set;
    } = new FixedList<DataTableColumn>();


    public FixedList<IOperationalReportEntryDto> Entries {
      get; internal set;
    } = new FixedList<IOperationalReportEntryDto>();

  } // class OperationalReportDto


  /// <summary>Output DTO used to return the entries of an operational report.</summary>
  public class OperationalReportEntryDto : IOperationalReportEntryDto {

    public string AccountNumber {
      get; internal set;
    }

    public decimal InitialBalance {
      get; internal set;
    }


    public decimal Debit {
      get; internal set;
    }


    public decimal Credit {
      get; internal set;
    }


    public decimal CurrentBalance {
      get; internal set;
    }


    public string UID {
      get; internal set;
    }


    public string AccountName {
      get; internal set;
    }


    public int AccountLevel {
      get; internal set;
    }


    public string Naturaleza {
      get; internal set;
    } = "D";


    public string CurrencyCode {
      get;
      internal set;
    }


    public string SectorCode {
      get;
      internal set;
    }


    public string GroupingCode {
      get;
      internal set;
    }

  } // class OperationalReportEntryDto

} // Empiria.FinancialAccounting.BanobrasIntegration.SATReports.Adapters
