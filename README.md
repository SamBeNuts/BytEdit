![BytEdit_logo](https://preview.ibb.co/j9Vs3x/logo_Byt_Edit.png)

# IMPORTANT
[Cahier des charges](https://docs.google.com/document/d/1uxTQMVv9ph9IEeWgpVHvYmsOsaD4I77LnrPwGH6l7eU/edit?usp=sharing)

## Règles à suivre
> Vouloir battre les 2 Juliens (et tous les autres au passage), ensuite vous pouvez lire la suite.

- Respecter la syntaxe
- Prendre le temps de commenter vos lignes de code
- Si vous voulez rajouter des images, faites-le dans le dossier `images` (à faire en cas de nécessité absolue)
- Ne jamais `commit` les fichiers `db` et `config`

### BDD (MySQL)
Règles à suivre pour initialiser la base de données :
1. Installer un serveur MySQL sur votre PC (par exemple celui inclut dans [WampServer](http://www.wampserver.com/), même si là nous n'utiliserons pas le dixième des services fournis)
2. Une fois installé, créer une base de données `bytedit` dans phpmyadmin avec `utf8_unicode_ci` comme interclassement
3. [Téléchargez ce fichier](https://drive.google.com/uc?id=1MeRYsX1JfWlvuPtKI1MOBM5E4BxO8QB5&export=download)
4. Une fois le fichier téléchargé, allez dans votre base de données et cliquez sur `Importer`, puis choisissez le fichier `bytedit.sql` et faites `exécuter`
5. Il vous reste à créer et configurer le fichier `db` dans le dossier `WPF_BytEdit`, pour cela vous pouvez procéder de 2 manières :
* Méthode 1 - Créer un fichier `db` et y écrire les données suivantes (sans faire d'espaces, seulement des sauts de ligne) :
```php
server=/*id de votre serveur*/
database=/*nom de la base de donnée (bytedit si vous n'avez pas modifié le fichier téléchargé à l'étape 3)*/
uid=/*votre identifiant*/
password=/*votre mot de passe*/
```
* Méthode 2 - Placer ces lignes de code avant la déclaration de la variable `connectionString` de la classe `App` du fichier `App.xaml.cs` lors et seulement lors de la première génération du projet :
```php
WriteData("db","server","/*id de votre serveur*/");
WriteData("db","database","/*nom de la base de donnée (bytedit si vous n'avez pas modifié le fichier téléchargé à l'étape 3)*/");
WriteData("db","uid","/*votre identifiant*/");
WriteData("db","password","/*votre mot de passe*/");
```
> Les commentaires `/*commentaire*/` sont bien évidemment à remplacer par vos informations personnelles.
6. Votre base de données est maintenant opérationnelle

#### Contact
Si vous rencontrez un problème ou si vous avez des questions :
* Nos mails : `samuel.lager@telecom-st-etienne.fr` et `thomas.arnette@telecom-st-etienne.fr`
