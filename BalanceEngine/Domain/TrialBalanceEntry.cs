/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Empiria Plain Object                    *
*  Type     : TrialBalanceEntry                          License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Represents an entry for a trial balance.                                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Represents an entry for a trial balance.</summary>
  public class TrialBalanceEntry {

    #region Constructors and parsers

    private TrialBalanceEntry() {
      // Required by Empiria Framework.
    }

    #endregion Constructors and parsers

    [DataField("ID_MAYOR", ConvertFrom = typeof(decimal))]
    private int _ledgerId = 0;

    public Ledger Ledger {
      get {
        return Ledger.Parse(_ledgerId);
      }
    }


    [DataField("ID_MONEDA", ConvertFrom = typeof(decimal))]
    private int currencyId = 0;
    public Currency Currency {
      get {
        return Currency.Parse(currencyId);
      }
    }


    [DataField("ID_CUENTA_ESTANDAR_HIST", ConvertFrom = typeof(long))]
    public Account Account {
      get;
      private set;
    }


    [DataField("ID_SECTOR", ConvertFrom = typeof(long))]
    private int _sectorId;


    public Sector Sector {
      get {
        if (_sectorId == 0) {
          _sectorId = -1;
        }
        return Sector.Parse(_sectorId);
      }
    }


    [DataField("SALDO_ANTERIOR")]
    public decimal InitialBalance {
      get;
      private set;
    }


    [DataField("DEBE")]
    public decimal Debit {
      get;
      private set;
    }


    [DataField("HABER")]
    public decimal Credit {
      get;
      private set;
    }


    [DataField("SALDO_ACTUAL")]
    public decimal CurrentBalance {
      get;
      private set;
    }


    public int Level {
      get {
        return EmpiriaString.CountOccurences(Account.Number, '-') + 1;
      }
    }

  } // class TrialBalance

} // namespace Empiria.FinancialAccounting.BalanceEngine
