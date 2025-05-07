﻿using LumeAI.Data;
using LumeAI.DTOs;
using LumeAI.Models.Movie;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace LumeAI.Services
{
    public class MovieJsonToRelational
    {
        private LumeAIDataContext _context;
        public MovieJsonToRelational(LumeAIDataContext context)
        {
            _context = context;
        }
        public void ConvertJsonToRelational(string jsonFilePath)
        {
            Console.WriteLine("Iniciando processo de mapear JSON para banco de dados relacional");
            var jsonContent = File.ReadAllText(jsonFilePath);
            var root = JsonSerializer.Deserialize<MovieExportRelational>(jsonContent);

            try
            {

                Console.WriteLine("Mapeando clusters");
                var clusterMap = new Dictionary<int, Cluster>();

                foreach (var clusterJson in root.Centroids)
                {
                    var clusterId = clusterJson.Id + 1;

                    // já criamos esse cluster nesta execução
                    if (clusterMap.ContainsKey(clusterId))
                        continue;

                    //// já está rastreado no contexto
                    //var existingCluster = _context.Clusters.Local
                    //    .FirstOrDefault(c => c.Id == clusterId);

                    //if (existingCluster is null)
                    //{
                    //    existingCluster = _context.Clusters
                    //        .FirstOrDefault(c => c.Id == clusterId);
                    //}

                    // não existe ainda no contexto, criar e adicionar
                    var newCluster = new Cluster
                    {
                        Id = clusterId,
                        CentroidVector = clusterJson.Centroid
                    };

                    _context.Clusters.Add(newCluster);
                    clusterMap[clusterId] = newCluster;
                }
                Console.WriteLine("Salvando clusters no banco de dados");

                _context.SaveChanges(); //  salva todos os clusters antes de continuar
                Console.WriteLine("Mapeamento de clusters concluido");

                Console.WriteLine("Iniciando mapeamento dos filmes");

                foreach (var movie in root.Movies)
                {
                    if (!clusterMap.TryGetValue((int)movie.ClusterId, out var cluster))
                    {
                        cluster = _context.Clusters.Find((int)movie.ClusterId);
                        if (cluster == null)
                        {
                            Console.WriteLine($"Cluster {movie.ClusterId} não encontrado.");
                            continue;
                        }

                        clusterMap[(int)movie.ClusterId] = cluster;
                    }

                    var movieId = int.Parse(movie.Id);

                    if (_context.Movies.Any(m => m.Id == movieId))
                        continue;

                    var movieEntity = new Movie
                    {
                        Id = movieId,
                        Title = movie.Title,
                        VoteAverage = movie.VoteAverage ?? 0,
                        VoteCount = movie.VoteCount ?? 0,
                        Status = movie.Status,
                        ReleaseDate = movie.ReleaseYear.HasValue ? new DateTime(movie.ReleaseYear.Value, 1, 1) : null,
                        Revenue = movie.Revenue ?? 0,
                        Runtime = movie.Runtime ?? 0,
                        Adult = movie.Adult ?? false,
                        BackdropPath = movie.BackdropPath,
                        Budget = movie.Budget ?? 0,
                        Homepage = movie.Homepage,
                        ImdbId = movie.ImdbId,
                        OriginalLanguage = movie.OriginalLanguage,
                        OriginalTitle = movie.OriginalTitle,
                        Overview = movie.Overview,
                        Popularity = movie.Popularity ?? 0,
                        PosterPath = movie.PosterPath,
                        Tagline = movie.Tagline,
                        ClusterId = (int)movie.ClusterId,
                        Cluster = cluster,

                        MovieGenres = new List<MovieGenre>(),
                        MovieKeywords = new List<MovieKeyword>(),
                        MovieProductionCompanies = new List<MovieProductionCompany>(),
                        MovieProductionCountries = new List<MovieProductionCountry>(),
                        MovieSpokenLanguages = new List<MovieSpokenLanguage>()
                    };

                    Console.WriteLine("Iniciando mapeamento dos gêneros");
                    foreach (var genreName in movie.Genres ?? [])
                    {
                        var genre = _context.Genres.FirstOrDefault(g => g.Name == genreName)
                                    ?? new Genre { Name = genreName };

                        if (genre.Id == 0) _context.Genres.Add(genre);
                        movieEntity.MovieGenres.Add(new MovieGenre { Genre = genre });
                    }

                    Console.WriteLine("Iniciando mapeamento das palavras-chave");
                    foreach (var keywordName in movie.Keywords ?? [])
                    {
                        var keyword = _context.Keywords.FirstOrDefault(k => k.Name == keywordName)
                                      ?? new Keyword { Name = keywordName };

                        if (keyword.Id == 0) _context.Keywords.Add(keyword);
                        movieEntity.MovieKeywords.Add(new MovieKeyword { Keyword = keyword });
                    }

                    Console.WriteLine("Iniciando mapeamento das companhias");
                    foreach (var companyName in movie.ProductionCompanies ?? [])
                    {
                        var company = _context.ProductionCompanies.FirstOrDefault(c => c.Name == companyName)
                                      ?? new ProductionCompany { Name = companyName };

                        if (company.Id == 0) _context.ProductionCompanies.Add(company);
                        movieEntity.MovieProductionCompanies.Add(new MovieProductionCompany { ProductionCompany = company });
                    }

                    Console.WriteLine("Iniciando mapeamento dos países");
                    foreach (var countryName in movie.ProductionCountries ?? [])
                    {
                        var country = _context.ProductionCountries.FirstOrDefault(c => c.Name == countryName)
                                      ?? new ProductionCountry { Name = countryName };

                        if (country.Id == 0) _context.ProductionCountries.Add(country);
                        movieEntity.MovieProductionCountries.Add(new MovieProductionCountry { ProductionCountry = country });
                    }

                    Console.WriteLine("Iniciando mapeamento dos idiomas");
                    foreach (var languageName in movie.SpokenLanguages ?? [])
                    {
                        var language = _context.SpokenLanguages.FirstOrDefault(l => l.Name == languageName)
                                       ?? new SpokenLanguage { Name = languageName };

                        if (language.Id == 0) _context.SpokenLanguages.Add(language);
                        movieEntity.MovieSpokenLanguages.Add(new MovieSpokenLanguage { SpokenLanguage = language });
                    }

                    _context.Movies.Add(movieEntity);
                }

                Console.WriteLine("Mapeamento em memória concluído, salvando alterações no banco de dados...");
                _context.SaveChanges();
                Console.WriteLine("Importação concluída com sucesso.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERRO: {ex.Message}");
                if (ex.InnerException is not null)
                {
                    Console.WriteLine($"ERRO INTERNO: {ex.InnerException.Message}");
                }
            }

        }
    }
}
