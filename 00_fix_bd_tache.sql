-- =========================================================
-- Script de correction pour bd_tache
-- A executer AVANT de lancer l'application C#
-- =========================================================

-- 1) La colonne "texte" d'un commentaire etait en INT -> impossible
--    d'y stocker du texte. On corrige en TEXT.
ALTER TABLE commentaire MODIFY texte TEXT NOT NULL;

-- 2) La colonne "date" du commentaire etait en INT -> on la renomme
--    et on la passe en DATE.
ALTER TABLE commentaire CHANGE `date` date_commentaire DATE NOT NULL;

-- 3) Il manquait la contrainte de cle etrangere entre commentaire.employe
--    et employe.idemploye (elle existait pour id_tache mais pas pour employe).
ALTER TABLE commentaire
  ADD CONSTRAINT commentaire_ibfk_2 FOREIGN KEY (employe) REFERENCES employe (idemploye)
  ON DELETE CASCADE ON UPDATE CASCADE;

-- 4) Auto-increment sur les cles primaires pour ne plus avoir a
--    generer les ID manuellement en C#.
ALTER TABLE employe MODIFY idemploye INT NOT NULL AUTO_INCREMENT;
ALTER TABLE tache MODIFY id_tache INT NOT NULL AUTO_INCREMENT;
ALTER TABLE commentaire MODIFY id_comment INT NOT NULL AUTO_INCREMENT;
ALTER TABLE notification MODIFY id_notif INT NOT NULL AUTO_INCREMENT;

-- 5) Ajout d'une colonne de suivi d'avancement sur la tache
--    (demande explicitement dans l'enonce, absente de la BD fournie).
ALTER TABLE tache
  ADD COLUMN statut VARCHAR(20) NOT NULL DEFAULT 'A faire';
  -- valeurs utilisees par l'appli : 'A faire', 'En cours', 'Terminee'

-- 6) La colonne "lu" de notification passe en booleen simplifie (0/1)
--    pour eviter les erreurs de comparaison de chaines.
ALTER TABLE notification MODIFY lu TINYINT(1) NOT NULL DEFAULT 0;

-- 7) La contrainte tache -> employe n'avait pas de ON DELETE CASCADE,
--    ce qui empeche de supprimer un employe ayant des taches.
ALTER TABLE tache DROP FOREIGN KEY tache_ibfk_1;
ALTER TABLE tache
  ADD CONSTRAINT tache_ibfk_1 FOREIGN KEY (idemploye) REFERENCES employe (idemploye)
  ON DELETE CASCADE ON UPDATE CASCADE;
