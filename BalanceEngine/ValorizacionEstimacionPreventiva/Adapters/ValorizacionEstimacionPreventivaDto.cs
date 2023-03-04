/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Data Transfer Object                    *
*  Type     : ValorizacionEstimacionPreventivaDto        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO used to return the entries of a valorization report                                 *
*             with foreign currencies totals and UDIS                                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

namespace Empiria.FinancialAccounting.BalanceEngine.Adapters {

  /// <summary>Output DTO used to return the entries of a valorization report
  /// with foreign currencies totals and UDIS</summary>
  public class ValorizacionEstimacionPreventivaDto {


    public TrialBalanceQuery Query {
      get; internal set;
    }


    public FixedList<DataTableColumn> Columns {
      get; internal set;
    }


    public FixedList<ValorizacionEntryDto> Entries {
      get; internal set;
    }


  } // class ValorizacionEstimacionPreventivaDto


  public class ValorizacionEntryDto : DynamicValorizacionEntryDto, ITrialBalanceEntryDto {

    public string CurrencyCode {
      get; internal set;
    }

    public int StandardAccountId {
      get; internal set;
    }

    public string AccountNumber {
      get; internal set;
    }

    public string AccountName {
      get; internal set;
    }


    public decimal MXN {
      get; internal set;
    }


    public decimal MXNDebit {
      get; internal set;
    }


    public decimal MXNCredit {
      get; internal set;
    }


    public DebtorCreditorType DebtorCreditor {
      get; internal set;
    } = DebtorCreditorType.Deudora;


    public decimal ValuedEffects {
      get; internal set;
    }

    public decimal TotalValued {
      get; internal set;
    }

    public decimal TotalAccumulated {
      get; internal set;
    }

    public decimal USD {
      get; internal set;
    }

    public decimal YEN {
      get; internal set;
    }

    public decimal EUR {
      get; internal set;
    }

    public decimal UDI {
      get; internal set;
    }

    public decimal LastUSD {
      get; internal set;
    }

    public decimal LastYEN {
      get; internal set;
    }

    public decimal LastEUR {
      get; internal set;
    }

    public decimal LastUDI {
      get; internal set;
    }

    public decimal ValuedUSD {
      get; internal set;
    }

    public decimal ValuedYEN {
      get; internal set;
    }

    public decimal ValuedEUR {
      get; internal set;
    }

    public decimal ValuedUDI {
      get; internal set;
    }

    public decimal ValuedEffectUSD {
      get; internal set;
    }

    public decimal ValuedEffectYEN {
      get; internal set;
    }

    public decimal ValuedEffectEUR {
      get; internal set;
    }

    public decimal ValuedEffectUDI {
      get; internal set;
    }

    public decimal ValuedExchangeRate {
      get; internal set;
    } = 1;
    public decimal USDDebit {
      get;
      internal set;
    }

    public decimal USDCredit {
      get;
      internal set;
    }

    public decimal YENDebit {
      get;
      internal set;
    }

    public decimal YENCredit {
      get;
      internal set;
    }

    public decimal EURDebit {
      get;
      internal set;
    }

    public decimal EURCredit {
      get;
      internal set;
    }

    public decimal UDIDebit {
      get;
      internal set;
    }

    public decimal UDICredit {
      get;
      internal set;
    }

    public decimal ValuedUSDDebit {
      get;
      internal set;
    }

    public decimal ValuedYENDebit {
      get;
      internal set;
    }

    public decimal ValuedEURDebit {
      get;
      internal set;
    }

    public decimal ValuedUDIDebit {
      get;
      internal set;
    }

    public decimal ValuedUSDCredit {
      get;
      internal set;
    }

    public decimal ValuedYENCredit {
      get;
      internal set;
    }

    public decimal ValuedEURCredit {
      get;
      internal set;
    }

    public decimal ValuedUDICredit {
      get;
      internal set;
    }

    public DateTime LastChangeDate {
      get; internal set;
    }

    public DateTime ConsultingDate {
      get; private set;
    }

    public TrialBalanceItemType ItemType {
      get; internal set;
    }

    public string SectorCode {
      get; internal set;
    }

    string ITrialBalanceEntryDto.SubledgerAccountNumber {
      get {
        return String.Empty;
      }
    }


    public override IEnumerable<string> GetDynamicMemberNames() {
      List<string> members = new List<string>();

      //members.Add("ItemType");
      //members.Add("CurrencyCode");
      //members.Add("StandardAccountId");
      //members.Add("AccountNumber");
      //members.Add("AccountName");
      //members.Add("USD");
      //members.Add("YEN");
      //members.Add("EUR");
      //members.Add("UDI");
      //members.Add("LastUSD");
      //members.Add("LastYEN");
      //members.Add("LastEUR");
      //members.Add("LastUDI");
      //members.Add("CurrentUSD");
      //members.Add("CurrentYEN");
      //members.Add("CurrentEUR");
      //members.Add("CurrentUDI");
      //members.Add("ValuedEffectUSD");
      //members.Add("ValuedEffectYEN");
      //members.Add("ValuedEffectEUR");
      //members.Add("ValuedEffectUDI");
      //members.Add("ValuedEffects");
      //members.Add("TotalValued");

      members.AddRange(base.GetDynamicMemberNames());

      //members.Add("TotalAccumulated");

      return members;
    }


  } // class ValorizacionEntryDto


  public class DynamicValorizacionEntryDto : DynamicFields {

  } // class DynamicValorizacionEntryDto


} // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters
