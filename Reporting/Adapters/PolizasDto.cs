/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                            Component : Interface adapters                   *
*  Assembly : FinancialAccounting.Reporting.dll             Pattern   : Data Transfer Object                 *
*  Type     : PolizasDto                                 License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO used to return vouchers report data.                                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Reporting.Adapters {

  public interface IPolizasDto {

  }

  /// <summary>Output DTO used to return vouchers report data.</summary>
  public class PolizasDto {

    public PolizasCommand Command {
      get; internal set;
    } = new PolizasCommand();


    public FixedList<DataTableColumn> Columns {
      get; internal set;
    } = new FixedList<DataTableColumn>();


    public FixedList<IPolizasDto> Entries {
      get; internal set;
    } = new FixedList<IPolizasDto>();

  } // class PolizasDto


  public class PolizasEntryDto : IPolizasDto {
    

    public string LedgerNumber {
      get; internal set;
    }


    public string LedgerName {
      get; internal set;
    }


    public string VoucherNumber {
      get;
      internal set;
    }


    public DateTime AccountingDate {
      get; internal set;
    }


    public DateTime RecordingDate {
      get; internal set;
    }


    public string ElaboratedBy {
      get; internal set;
    }


    public string Concept {
      get; internal set;
    }


    public decimal Debit {
      get; internal set;
    }


    public decimal Credit {
      get; internal set;
    }
    
  } // class PolizasEntryDto

} // namespace Empiria.FinancialAccounting.Reporting.Adapters
