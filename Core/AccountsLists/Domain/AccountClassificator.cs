/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Lists                             Component : Domain Layer                            *
*  Assembly : Empiria.FinancialAccounting.dll            Pattern   : Information holder                      *
*  Type     : AccountClassificator                       License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Describes a financial account with a set of classificators.                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System;

using Empiria.DataTypes;
using Empiria.Json;
using Empiria.StateEnums;

namespace Empiria.FinancialAccounting.AccountsLists {

  /// <summary>Describes a financial account with a set of classificators.</summary>
  public class AccountClassificator : BaseObject, IAccountListItem {

    #region Constructors and parsers

    private AccountClassificator() {
      // Required by Empiria Framework.
    }

    static internal AccountClassificator Parse(int id) => ParseId<AccountClassificator>(id);

    static internal AccountClassificator Parse(string uid) => ParseKey<AccountClassificator>(uid);

    #endregion Constructors and parsers

    #region Properties

    [DataField("ID_LISTA")]
    internal int ListId {
      get; private set;
    }


    public Account Account {
      get {
        return AccountsChart.IFRS.GetAccount(AccountNumber);
      }
    }


    [DataField("NUMERO_CUENTA_ESTANDAR")]
    public string AccountNumber {
      get; private set;
    }


    [DataField("FECHA_INICIO")]
    public DateTime StartDate {
      get;
      private set;
    }


    [DataField("FECHA_FIN")]
    public DateTime EndDate {
      get;
      private set;
    }


    public string Keywords {
      get {
        return EmpiriaString.BuildKeywords(AccountNumber, Account.Name);
      }
    }

    public FixedList<KeyValue> Classificators {
      get {
        return ExtData.GetFixedList<KeyValue>("classificators");
      }
    }

    [DataField("ELEMENTO_EXT_DATA")]
    private JsonObject ExtData {
      get; set;
    }

    [DataField("STATUS_ELEMENTO_LISTA", Default = EntityStatus.Active)]
    public EntityStatus Status {
      get;
      private set;
    } = EntityStatus.Active;


    #endregion Properties

    public string GetClassificatorValue(string classKey) {
      Assertion.Require(classKey, nameof(classKey));

      string value = TryGetClassificatorValue(classKey);

      Assertion.Require(value,
        $"A classificator with key '{classKey}' is not defined for account {AccountNumber}");

      return value;
    }


    public string TryGetClassificatorValue(string classKey) {
      Assertion.Require(classKey, nameof(classKey));

      var classificator = Classificators.Find(x => x.Key == classKey);

      return classificator?.Value;
    }

  }  // class AccountClassificatorListItem

}  // namespace Empiria.FinancialAccounting.AccountsLists
