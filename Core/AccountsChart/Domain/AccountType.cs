/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Chart                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Empiria General Object                  *
*  Type     : AccountType                                License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Describes the type or use of an account (e.g. activo, pasivo, capital, orden, etc).            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.FinancialAccounting {

  /// <summary>Describes the type or use of an account (e.g. activo, pasivo, capital, orden, etc).</summary>
  public class AccountType : GeneralObject {

    private AccountType() {
      // Required by Empiria Framework.
    }

    static public AccountType Parse(int id) {
      return BaseObject.ParseId<AccountType>(id);
    }

    static public AccountType Empty => BaseObject.ParseEmpty<AccountType>();

  } // class AccountType

}  // namespace Empiria.FinancialAccounting
