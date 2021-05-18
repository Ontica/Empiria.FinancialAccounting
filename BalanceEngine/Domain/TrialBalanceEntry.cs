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
  internal class TrialBalanceEntry {

    #region Constructors and parsers

    private TrialBalanceEntry() {
      // Required by Empiria Framework.
    }

    #endregion Constructors and parsers

    [DataField("ID_MAYOR", ConvertFrom = typeof(long))]
    public int LedgerId {
      get;
      private set;
    } = -1;

    [DataField("ID_MONEDA", ConvertFrom = typeof(long))]
    public int CurrencyId {
      get;
      private set;
    } = -1;

    [DataField("ID_CUENTA", ConvertFrom = typeof(long))]
    public int LedgerAccountId {
      get;
      private set;
    } = -1;

    [DataField("ID_CUENTA_ESTANDAR", ConvertFrom = typeof(long))]
    public int AccountId {
      get;
      private set;
    } = -1;

    [DataField("ID_SECTOR", ConvertFrom = typeof(long))]
    public int SectorId {
      get;
      private set;
    } = -1;

    [DataField("NUMERO_CUENTA_ESTANDAR")]
    public string AccountNumber {
      get;
      private set;
    }

    [DataField("NOMBRE_CUENTA_ESTANDAR")]
    public string AccountName {
      get;
      private set;
    }


    //[DataField("ID_CUENTA_AUXILIAR")]
    //public int SubsidiaryAccountId {
    //  get;
    //  private set;
    //}

    //[DataField("CLAVE_PRESUPUESTAL")]
    //public string BudgetKey {
    //  get;
    //  private set;
    //}

    //[DataField("NUMERO_CUENTA_AUXILIAR")]
    //public string SubsidiaryAccountNumber {
    //  get;
    //  private set;
    //}

    //[DataField("NOMBRE_CUENTA_AUXILIAR")]
    //public string SubsidiaryAccountName {
    //  get;
    //  private set;
    //}

    //[DataField("ID_TIPO_CUENTA")]
    //public int AccountTypeId {
    //  get;
    //  private set;
    //}

    //[DataField("NATURALEZA")]
    //public string Naturaleza {
    //  get;
    //  private set;
    //}

    //[DataField("ROL_CUENTA")]
    //public char AccountRole {
    //  get;
    //  private set;
    //}

    [DataField("SALDO_ANTERIOR")]
    public decimal PreviousBalance {
      get;
      private set;
    }

    [DataField("CARGOS")]
    public decimal Debit {
      get;
      private set;
    }

    [DataField("ABONOS")]
    public decimal Credit {
      get;
      private set;
    }

    [DataField("SALDO_ACTUAL")]
    public decimal CurrentBalance {
      get;
      private set;
    }

  } // class TrialBalance

} // namespace Empiria.FinancialAccounting.BalanceEngine
