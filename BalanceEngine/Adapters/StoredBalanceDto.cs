/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                               Component : Domain Layer                          *
*  Assembly : FinancialAccounting.BalanceEngine.dll        Pattern   : Information Holder                    *
*  Type     : StoredBalanceDto                             License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Holds a stored account balance.                                                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.BalanceEngine {

  public class StoredBalanceDto {

    internal StoredBalanceDto() {
      // no-op
    }

    public int StandardAccountId {
      get; internal set;
    }

    public int LedgerAccountId {
      get; internal set;
    }

    public NamedEntityDto Ledger {
      get; internal set;
    }

    public NamedEntityDto Currency {
      get; internal set;
    }

    public int SubsidiaryAccountId {
      get; internal set;
    }

    public string SectorCode {
      get; internal set;
    }

    public string AccountName {
      get; internal set;
    }

    public string AccountNumber {
      get; internal set;
    }

    public string SubsidiaryAccountNumber {
      get; internal set;
    }

    public string SubsidiaryAccountName {
      get; internal set;
    }

    public decimal Balance {
      get; internal set;
    }


  }  // class StoredBalance

}  // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters
