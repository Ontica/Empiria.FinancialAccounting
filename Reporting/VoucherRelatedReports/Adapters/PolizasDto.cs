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
using Empiria.FinancialAccounting.Reporting.VoucherRelatedReports.Domain;
using Newtonsoft.Json;

namespace Empiria.FinancialAccounting.Reporting.VoucherRelatedReports.Adapters {

  public interface IPolizasDto {

  }

  /// <summary>Output DTO used to return vouchers report data.</summary>
  public class PolizasDto {

    [JsonProperty]
    public ListadoPolizasQuery Query {
      get; internal set;
    } = new ListadoPolizasQuery();


    [JsonProperty]
    public FixedList<DataTableColumn> Columns {
      get; internal set;
    } = new FixedList<DataTableColumn>();


    [JsonProperty]
    public FixedList<IPolizasDto> Entries {
      get; internal set;
    } = new FixedList<IPolizasDto>();

  } // class PolizasDto


  /// <summary>DTO for each poliza entry report.</summary>
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


    public int VouchersByLedger {
      get; internal set;
    }


    public ItemType ItemType {
      get; internal set;
    } = ItemType.Entry;

  } // class PolizasEntryDto

} // namespace Empiria.FinancialAccounting.Reporting.VoucherRelatedReports.Adapters
