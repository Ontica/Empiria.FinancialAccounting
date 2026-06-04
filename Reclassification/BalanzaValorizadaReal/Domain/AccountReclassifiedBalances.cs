/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reclassification Services                  Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Reclassification.dll   Pattern   : Information Holder                      *
*  Type     : AccountReclassifiedBalances                License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Holds an account balances with registered and reclassified values.                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.FinancialAccounting.Reclassification {

  /// <summary>Holds an account balances with registered and reclassified values.</summary>
  public class AccountReclassifiedBalances {

    #region Constructors and Parsers

    internal AccountReclassifiedBalances() {
      // Required by Empiria Framework
    }

    #endregion Constructors and Parsers

    #region Properties

    [DataField("ID_CUENTA_ESTANDAR")]
    public StandardAccount StdAccount {
      get; private set;
    }

    [DataField("ID_MONEDA")]
    public Currency Currency {
      get; private set;
    }


    [DataField("SALDO_INICIAL", ConvertFrom = typeof(decimal))]
    public decimal InitialBalance {
      get; private set;
    }

    [DataField("DEBE", ConvertFrom = typeof(decimal))]
    public decimal Debits {
      get; private set;
    }

    [DataField("HABER", ConvertFrom = typeof(decimal))]
    public decimal Credits {
      get; private set;
    }

    public decimal FinalBalance {
      get {
        if (StdAccount.DebtorCreditor == DebtorCreditorType.Deudora) {
          return InitialBalance + Debits - Credits;
        } else {
          return InitialBalance - Debits + Credits;
        }
      }
    }


    [DataField("ID_MONEDA_REAL")]
    public Currency RealCurrency {
      get; private set;
    }


    [DataField("SALDO_INICIAL_REAL", ConvertFrom = typeof(decimal))]
    public decimal RealInitialBalance {
      get; private set;
    }


    [DataField("DEBE_MONEDA_REAL", ConvertFrom = typeof(decimal))]
    public decimal RealDebits {
      get; private set;
    }


    [DataField("HABER_MONEDA_REAL", ConvertFrom = typeof(decimal))]
    public decimal RealCredits {
      get; private set;
    }


    public decimal RealFinalBalance {
      get {
        if (StdAccount.DebtorCreditor == DebtorCreditorType.Deudora) {

          return RealInitialBalance + RealDebits - RealCredits;

        } else {

          return RealInitialBalance - RealDebits + RealCredits;
        }
      }
    }

    #endregion Properties

  } // class AccountReclassifiedBalances

} // namespace Empiria.FinancialAccounting.Reclassification
