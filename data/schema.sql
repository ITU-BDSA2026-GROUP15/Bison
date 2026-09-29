drop table if exists user;
create table user (
  user_id integer primary key autoincrement,
  username string not null,
  email string not null
);

drop table if exists observation;
create table observation (
  observation_id integer primary key autoincrement,
  author_id integer not null,
  text string not null,
  pub_date integer
);

drop table if exists proposal;
create table proposal (
  proposal_id integer, 
  author_id integer not null, 
  pub_date integer,
  primary key (proposal_id, pub_date)
);

drop table if exists comment;
create table comment (
  comment_id integer, 
  author_id integer not null,
  text string not null,
  pub_date integer,
  primary key (comment_id, pub_date)
);