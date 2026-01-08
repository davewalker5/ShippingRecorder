SELECT  o.Name,
        COUNT( DISTINCT s.Id ) AS "Sightings",
        COUNT( DISTINCT l.Id ) AS "Locations",
        COUNT( DISTINCT v.Id ) AS "Vessels"
FROM SIGHTING s
INNER JOIN LOCATION l ON l.Id = s.Location_Id
INNER JOIN VESSEL v ON v.Id = s.Vessel_Id
INNER JOIN REGISTRATION_HISTORY rh ON rh.Vessel_Id = v.Id
INNER JOIN OPERATOR o ON o.Id = rh.Operator_Id
WHERE s.Date BETWEEN '$from' AND '$to'
GROUP BY o.Name
ORDER BY o.Name ASC;
