SELECT  s.Date,
        l.Name AS "Location",
        v.IMO,
        rh.Name AS "Vessel",
        v.Id AS "VesselId"
FROM SIGHTING s
INNER JOIN LOCATION l ON l.Id = s.Location_Id
INNER JOIN VESSEL v ON v.Id = s.Vessel_Id
LEFT OUTER JOIN REGISTRATION_HISTORY rh ON rh.Vessel_Id = v.Id AND rh.Is_Active = 1
WHERE s.Is_My_Voyage = 1
AND s.Date BETWEEN '$from' AND '$to'
ORDER BY s.Date ASC;
