/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                            Component : Interface adapters                   *
*  Assembly : FinancialAccounting.Reporting.dll             Pattern   : Data Transfer Object                 *
*  Type     : IntegracionSaldosCapitalInteresesDto          License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Output DTO used to return 'Integración de saldos de capital e intereses' report data.          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Reporting.IntegracionSaldosCapitalIntereses.Adapters {

  public interface IIntegracionSaldosCapitalInteresesDto : IReportEntryDto {

  }

  public class IntegracionSaldosCapitalInteresesTitleDto : IIntegracionSaldosCapitalInteresesDto {

    public string ItemType {
      get; internal set;
    } = "Total";

    public string SubledgerAccount {
      get; set;
    }

  }  // class IntegracionSaldosCapitalInteresesTitleDto


  /// <summary>DTO for each account report entry.</summary>
  public class IntegracionSaldosCapitalInteresesEntryDto : IIntegracionSaldosCapitalInteresesDto {

    public string ItemType {
      get; internal set;
    } = "Entry";


    public string SubledgerAccount {
      get; internal set;
    } = string.Empty;


    public string SubledgerAccountName {
      get; internal set;
    } = string.Empty;


    public string PrestamoName {
      get; internal set;
    } = string.Empty;


    public string CurrencyCode {
      get; internal set;
    } = string.Empty;


    public string SectorCode {
      get; internal set;
    } = string.Empty;


    public decimal CapitalCortoPlazoMonedaOrigen {
      get; internal set;
    }

    public decimal CapitalLargoPlazoMonedaOrigen {
      get; internal set;
    }

    public decimal CapitalMonedaOrigenTotal {
      get; internal set;
    }

    public decimal InteresesMonedaOrigenTotal {
      get; internal set;
    }

    public decimal TotalMonedaOrigen {
      get; internal set;
    }

    public decimal TipoCambio {
      get; internal set;
    }

    public decimal CapitalMonedaNacional {
      get; internal set;
    }

    public decimal InteresesMonedaNacional {
      get; internal set;
    }

    public decimal TotalMonedaNacional {
      get; internal set;
    }

    public DateTime? Vencimiento {
      get; internal set;
    }

  } // class IntegracionSaldosCapitalInteresesEntryDto



  public class IntegracionSaldosCapitalInteresesTotalDto : IIntegracionSaldosCapitalInteresesDto {

    public string ItemType {
      get; internal set;
    } = "Total";


    public string SubledgerAccount {
      get;
      internal set;
    }

    public decimal CapitalMonedaNacional {
      get; internal set;
    }

    public decimal InteresesMonedaNacional {
      get; internal set;
    }

    public decimal TotalMonedaNacional {
      get; internal set;
    }

  }  // class IntegracionSaldosCapitalInteresesTotalDto

} // namespace Empiria.FinancialAccounting.Reporting.IntegracionSaldosCapitalIntereses.Adapters
