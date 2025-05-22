﻿/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Catalogues                                 Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Empiria Data Object                     *
*  Type     : Participant                                License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Legacy entity that describes an accounting system user or organization.                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.FinancialAccounting.Data;

namespace Empiria.FinancialAccounting {

  /// <summary>Legacy entity that describes an accounting system user or organization.</summary>
  public class Participant : BaseObject {

    #region Constructors and parsers

    protected Participant() {
      // Required by Empiria Framework.
    }

    static public Participant Parse(int id) {
      return BaseObject.ParseId<Participant>(id);
    }


    static public Participant TryParse(string uid) {
      return BaseObject.TryParse<Participant>($"UPPER(ParticipantKey) = '{uid.ToUpperInvariant()}'");
    }


    static public FixedList<Participant> GetList() {
      string filter = "ParticipantType = 'U' AND Status = 'A'";
      string orderBy = "ParticipantName";

      return BaseObject.GetList<Participant>(filter, orderBy).ToFixedList();
    }


    static public FixedList<Participant> GetList(string keywords) {
      Assertion.Require(keywords, "keywords");

      string filter = "ParticipantType = 'U' AND Status = 'A' AND "+
                      SearchExpression.ParseAndLike("Keywords", keywords);

      return ParticipantData.SearchParticipants(filter);
    }


    static public Participant Empty {
      get {
        return BaseObject.ParseEmpty<Participant>();
      }
    }

    public static Participant Current {
      get {
        return Participant.Parse(ExecutionServer.CurrentUserId);
      }
    }

    #endregion Constructors and parsers

    #region Properties

    [DataField("ParticipantName")]
    public string Name {
      get; private set;
    }

    [DataField("ParticipantKey")]
    public string UserName {
      get; private set;
    }


    #endregion Properties

    #region Methods

    public NamedEntityDto MapToNamedEntity() {
      return new NamedEntityDto(this.Id.ToString(), $"[{this.UserName}] {this.Name}");
    }


    #endregion Methods

  } // class Participant

}  // namespace Empiria.FinancialAccounting
