/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Empiria Plain Object                    *
*  Type     : CoreBalanceEntry                           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Represents a core balance entry.                                                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Represents a core balance entry.</summary>
  public class CoreBalanceEntry {

    #region Constructors and parsers

    internal CoreBalanceEntry() {
      // Required by Empiria Framework.
    }

    #endregion Constructors and parsers

    [DataField("ID_MAYOR", ConvertFrom = typeof(long))]
    public Ledger Ledger {
      get; private set;
    }


    [DataField("ID_MONEDA", ConvertFrom = typeof(long))]
    public Currency Currency {
      get; private set;
    }


    [DataField("ID_CUENTA_ESTANDAR", ConvertFrom = typeof(long))]
    public StandardAccount Account {
      get; private set;
    }


    [DataField("ID_SECTOR", ConvertFrom = typeof(long))]
    public Sector Sector {
      get; private set;
    }


    [DataField("ID_CUENTA_AUXILIAR", ConvertFrom = typeof(long))]
    private int _subledgerAccountId = -1;

    public SubledgerAccount SubledgerAccount {
      get {
        return SubledgerAccount.Parse(_subledgerAccountId);
      }
    }


    [DataField("SALDO_ANTERIOR")]
    public decimal InitialBalance {
      get; private set;
    }


    [DataField("DEBE")]
    public decimal Debit {
      get; private set;
    }


    [DataField("HABER")]
    public decimal Credit {
      get; private set;
    }


    [DataField("SALDO_ACTUAL")]
    public decimal CurrentBalance {
      get; private set;
    }


    [DataField("SALDO_PROMEDIO")]
    public decimal AverageBalance {
      get; private set;
    }


    [DataField("FECHA_ULTIMO_MOVIMIENTO")]
    public DateTime LastChangeDate {
      get; private set;
    }


    public decimal ExchangeRate {
      get; private set;
    } = 1;


    internal void ValuateTo(decimal exchangeRate) {
      Assertion.Require(exchangeRate > 0, nameof(exchangeRate));

      this.ExchangeRate = exchangeRate;
      this.InitialBalance = Math.Round(InitialBalance * exchangeRate, 2);
      this.Debit = Math.Round(Debit * exchangeRate, 2);
      this.Credit = Math.Round(Credit * exchangeRate, 2);
      this.CurrentBalance = Math.Round(CurrentBalance * exchangeRate, 2);
      this.AverageBalance = Math.Round(AverageBalance * exchangeRate, 2);
    }

  } //class CoreBalanceEntry

} // namespace Empiria.FinancialAccounting.BalanceEngine
