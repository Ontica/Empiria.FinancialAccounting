/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                         Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Reporting.dll          Pattern   : Cache                                   *
*  Type     : AccountStatementCache                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides a cache of account statements.                                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Collections;
using Empiria.Json;
using Empiria.Security;

using Empiria.FinancialAccounting.Reporting.Adapters;

namespace Empiria.FinancialAccounting.Reporting.AccountStatements {

  /// <summary>Provides a cache of account statements.</summary>
  static internal class AccountStatementCache {

    static readonly EmpiriaHashTable<AccountStatement> _cache = new EmpiriaHashTable<AccountStatement>();


    static internal string GenerateHash(AccountStatementQuery command) {
      string json = JsonConverter.ToJson(command);

      return Cryptographer.CreateHashCode(json);
    }


    static internal AccountStatement TryGet(string cacheHashKey) {
      if (_cache.ContainsKey(cacheHashKey)) {
        return _cache[cacheHashKey];
      }

      return null;
    }


    static internal void Store(string cacheHashKey, AccountStatement accountStatement) {
      _cache.Insert(cacheHashKey, accountStatement);
    }

  } // class AccountStatementCache

} // namespace Empiria.FinancialAccounting.Reporting
