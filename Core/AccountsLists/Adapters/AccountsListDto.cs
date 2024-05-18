/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Lists                             Component : Interface adapters                      *
*  Assembly : Empiria.FinancialAccounting.dll            Pattern   : Data Transfer Object                    *
*  Type     : AccountsListDto                            License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO for accounts lists.                                                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.DynamicData;

namespace Empiria.FinancialAccounting.AccountsLists.Adapters {

  /// <summary>Output DTO for accounts lists.</summary>
  public class AccountsListDto {

    internal AccountsListDto() {
      // no-op
    }

    public string UID {
      get; internal set;
    }


    public string Name {
      get; internal set;
    }


    public FixedList<DataTableColumn> Columns {
      get; internal set;
    }


    public FixedList<AccountsListItemDto> Entries {
      get; internal set;
    }

  }  // class AccountsListDto


  public class AccountsListItemDto {


    public string UID {
      get;
      internal set;
    }

  }  // class AccountsListItemDto



  public class ConciliacionDerivadosListItemDto : AccountsListItemDto {

    public string AccountUID {
      get; internal set;
    }

    public string AccountNumber {
      get; internal set;
    }

    public string AccountName {
      get; internal set;
    }

    public DateTime StartDate {
      get; internal set;
    }

    public DateTime EndDate {
      get; internal set;
    }

  }  // class ConciliacionDerivadosListItemDto



  public class SwapsCoberturaListItemDto : AccountsListItemDto {

    public int SubledgerAccountId {
      get; internal set;
    }

    public string SubledgerAccountName {
      get; internal set;
    }

    public string SubledgerAccountNumber {
      get; internal set;
    }

    public string Classification {
      get; internal set;
    }

    public DateTime StartDate {
      get; internal set;
    }

    public DateTime EndDate {
      get; internal set;
    }

  }  // class SwapsCoberturaListItemDto



  public class DepreciacionActivoFijoListItemDto : AccountsListItemDto {

    public int AuxiliarHistoricoId {
      get; internal set;
    }

    public string AuxiliarHistoricoNombre {
      get; internal set;
    }

    public string AuxiliarHistorico {
      get; internal set;
    }

    public string NumeroInventario {
      get; internal set;
    }

    public string TipoActivoFijoUID {
      get; internal set;
    }

    public string TipoActivoFijoName {
      get; internal set;
    }

    public string NumeroDelegacion {
      get; internal set;
    }

    public string Delegacion {
      get; internal set;
    }

    public string DelegacionUID {
      get; internal set;
    }

    public DateTime FechaAdquisicion {
      get; internal set;
    }

    public DateTime FechaInicioDepreciacion {
      get; internal set;
    }

    public int MesesDepreciacion {
      get; internal set;
    }

    public string AuxiliarRevaluacion {
      get; internal set;
    } = string.Empty;


    public string AuxiliarRevaluacionNombre {
      get; internal set;
    } = string.Empty;


    public int AuxiliarRevaluacionId {
      get; internal set;
    } = -1;


    public decimal MontoRevaluacion {
      get; internal set;
    }

  }  // class DepreciacionActivoFijoListItemDto



  public class PrestamosInterbancariosListItemDto : AccountsListItemDto {

    public int SubledgerAccountId {
      get; internal set;
    }

    public string SubledgerAccountName {
      get; internal set;
    }

    public string SubledgerAccountNumber {
      get; internal set;
    }

    public string SectorCode {
      get; internal set;
    }

    public string CurrencyCode {
      get; internal set;
    }

    public string PrestamoUID {
      get; internal set;
    }

    public string PrestamoName {
      get; internal set;
    }

    public DateTime Vencimiento {
      get; internal set;
    }

  }  // class PrestamosInterbancariosListItemDto

}  // namespace Empiria.FinancialAccounting.Adapters
