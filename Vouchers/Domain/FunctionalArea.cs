/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Vouchers Management                        Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Vouchers.dll           Pattern   : Empiria Data Object                     *
*  Type     : FunctionalArea                             License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Holds data about a functional area.                                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System;
using Empiria.Data;

namespace Empiria.FinancialAccounting.Vouchers {

  /// <summary>Holds data about a functional area.</summary>
  public class FunctionalArea : BaseObject {

    #region Constructors and parsers

    protected FunctionalArea() {
      // Required by Empiria Framework.
    }


    static public FunctionalArea Parse(int id) {
      return BaseObject.ParseId<FunctionalArea>(id);
    }


    static public FunctionalArea Parse(string areaCode) {
      var area = FunctionalArea.TryParse(areaCode);

      Assertion.Require(area, $"Área funcional no encontrada {areaCode}");

      return area;
    }


    static public FunctionalArea ParseActive(string areaCode,
                                             DateTime applicationDate) {

      applicationDate = applicationDate.Date;

      var filter = $"ParticipantType = 'O' AND ParticipantKey = '{areaCode}' AND " +
                   $"Status = 'A' AND " +
                   $"FromDate <= '{DataCommonMethods.FormatSqlDbDate(applicationDate)}' AND " +
                   $"'{DataCommonMethods.FormatSqlDbDate(applicationDate)}' <= ToDate";

      var area = BaseObject.TryParse<FunctionalArea>(filter);

      Assertion.Require(area, $"No se encontró un área funcional activa con clave: {areaCode}");

      return area;
    }

    static public FunctionalArea TryParse(string areaCode) {

      var filter = $"ParticipantType = 'O' AND ParticipantKey = '{areaCode}' AND Status = 'A'";

      return BaseObject.TryParse<FunctionalArea>(filter);
    }


    static public FixedList<FunctionalArea> GetList() {
      string filter = "ParticipantType = 'O' AND Status = 'A'";
      string orderBy = "ParticipantKey";

      return BaseObject.GetList<FunctionalArea>(filter, orderBy)
                       .ToFixedList();
    }


    static public FunctionalArea Empty {
      get {
        return FunctionalArea.ParseEmpty<FunctionalArea>();
      }
    }

    #endregion Constructors and parsers

    #region Properties

    [DataField("ParticipantName")]
    public string Name {
      get; private set;
    }


    [DataField("ParticipantKey")]
    public string Code {
      get; private set;
    }


    [DataField("FromDate")]
    public DateTime FromDate {
      get; private set;
    }


    [DataField("ToDate")]
    public DateTime ToDate {
      get; private set;
    }


    public string FullName {
      get {
        return $"({Code}) {Name}";
      }
    }

    #endregion Properties

    public NamedEntityDto MapToNamedEntity() {
      return new NamedEntityDto(this.Id.ToString(), this.FullName);
    }

  } // class FunctionalArea

}  // namespace Empiria.FinancialAccounting.Vouchers
