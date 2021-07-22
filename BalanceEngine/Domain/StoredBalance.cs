/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                               Component : Domain Layer                          *
*  Assembly : FinancialAccounting.BalanceEngine.dll        Pattern   : Information Holder                    *
*  Type     : StoredBalance                                License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Hald aa stored account balance.                                                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.FinancialAccounting.BalanceEngine.Data;

namespace Empiria.FinancialAccounting.BalanceEngine {

  public class StoredBalance : BaseObject {

    private StoredBalance() {
      // Required by Empiria Framework
    }

    internal StoredBalance(StoredBalanceSet storedBalanceSet, TrialBalanceEntry entry) {
      var subsidiaryAccount = SubsidiaryAccount.Parse(entry.SubledgerAccountId);

      this.StoredBalanceSet = storedBalanceSet;
      this.StandardAccountId = entry.Account.Id;
      this.AccountNumber = entry.Account.Number;
      this.AccountName = entry.Account.Name;
      this.Ledger = entry.Ledger;
      this.LedgerAccountId = entry.AccountId;
      this.Currency = entry.Currency;
      this.Sector = entry.Sector;
      this.SubsidiaryAccountId = entry.SubledgerAccountId;
      this.SubsidiaryAccountName = subsidiaryAccount.Name;
      this.SubsidiaryAccountNumber = subsidiaryAccount.Number;
      this.Balance = entry.CurrentBalance;
    }

    [DataField("ID_GRUPO_SALDOS", ConvertFrom = typeof(long))]
    internal StoredBalanceSet StoredBalanceSet {
      get; private set;
    }


    [DataField("ID_CUENTA_ESTANDAR", ConvertFrom = typeof(long))]
    public int StandardAccountId {
      get; private set;
    }


    [DataField("NUMERO_CUENTA_ESTANDAR")]
    public string AccountNumber {
      get; private set;
    }


    [DataField("NOMBRE_CUENTA_ESTANDAR")]
    public string AccountName {
      get; private set;
    }


    [DataField("ID_MAYOR", ConvertFrom = typeof(long))]
    public Ledger Ledger {
      get; private set;
    }


    [DataField("ID_CUENTA", ConvertFrom = typeof(long))]
    public int LedgerAccountId {
      get; private set;
    }


    [DataField("ID_MONEDA", ConvertFrom = typeof(long))]
    public Currency Currency {
      get; private set;
    }


    [DataField("ID_SECTOR", ConvertFrom = typeof(long))]
    public Sector Sector {
      get; private set;
    }


    [DataField("ID_CUENTA_AUXILIAR", ConvertFrom = typeof(long))]
    public int SubsidiaryAccountId {
      get; private set;
    }


    [DataField("NUMERO_CUENTA_AUXILIAR")]
    public string SubsidiaryAccountNumber {
      get; private set;
    }


    [DataField("NOMBRE_CUENTA_AUXILIAR")]
    public string SubsidiaryAccountName {
      get; private set;
    }


    [DataField("SALDO_INICIAL")]
    public decimal Balance {
      get; private set;
    }


    protected override void OnSave() {
      StoredBalanceDataService.WriteStoredBalance(this);
    }

  }  // class StoredBalance


}  // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters
