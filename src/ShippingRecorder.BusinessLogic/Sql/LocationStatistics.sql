SELECT  l.Name,
        COUNT( DISTINCT s.Id ) AS "Sightings",
        COUNT( DISTINCT v.Id ) AS "Vessels"
FROM SIGHTING s
INNER JOIN LOCATION l ON l.Id = s.Location_Id
INNER JOIN VESSEL v ON v.Id = s.Vessel_Id
WHERE s.Date BETWEEN '$from' AND '$to'
GROUP BY l.Name
ORDER BY l.Name ASC;