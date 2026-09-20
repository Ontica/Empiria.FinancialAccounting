/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Transactions Services            Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Transactions.dll       Pattern   : Common storage type                     *
*  Type     : FinancialTransactionType                   License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Represents the type of a financial transaction.                                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.FinancialAccounting.Transactions {

  /// <summary>Represents the type of a financial transaction.</summary>
  public class FinancialTransactionType : CommonStorage {

    #region Constructors and parsers

    static public FinancialTransactionType Parse(int id) => ParseId<FinancialTransactionType>(id);

    static public FinancialTransactionType Parse(string uid) => ParseKey<FinancialTransactionType>(uid);

    static public FinancialTransactionType ParseKey(string key) => ParseNamedKey<FinancialTransactionType>(key);

    static public FixedList<FinancialTransactionType> GetList() => GetStorageObjects<FinancialTransactionType>();

    #endregion Constructors and parsers

    #region Properties

    public string Key {
      get {
        return base.Code;
      }
    }

    #endregion Properties

  }  // class FinancialTransactionType

}  // namespace Empiria.FinancialAccounting.Transactions
