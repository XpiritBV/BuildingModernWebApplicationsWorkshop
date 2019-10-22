/* tslint:disable */
import { Injectable } from '@angular/core';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { BaseService } from '../base-service';
import { ApiConfiguration } from '../api-configuration';
import { StrictHttpResponse } from '../strict-http-response';
import { RequestBuilder } from '../request-builder';
import { Observable } from 'rxjs';
import { map, filter } from 'rxjs/operators';

import { HighScore } from '../models/high-score';


/**
 * API to retrieve high score leaderboard
 */
@Injectable({
  providedIn: 'root',
})
export class LeaderboardService extends BaseService {
  constructor(
    config: ApiConfiguration,
    http: HttpClient
  ) {
    super(config, http);
  }

  /**
   * Path part for operation leaderboardGet
   */
  static readonly LeaderboardGetPath = '/api/v1.0/Leaderboard/{format}';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `leaderboardGet()` instead.
   *
   * This method doesn't expect any response body
   */
  leaderboardGet$Response(params: {
    version: string;
    format: string;

  }): Observable<StrictHttpResponse<Array<HighScore>>> {

    const rb = new RequestBuilder(this.rootUrl, LeaderboardService.LeaderboardGetPath, 'get');
    if (params) {

      rb.path('version', params.version);
      rb.path('format', params.format);

    }
    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'application/json'
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<Array<HighScore>>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `leaderboardGet$Response()` instead.
   *
   * This method doesn't expect any response body
   */
  leaderboardGet(params: {
    version: string;
    format: string;

  }): Observable<Array<HighScore>> {

    return this.leaderboardGet$Response(params).pipe(
      map((r: StrictHttpResponse<Array<HighScore>>) => r.body as Array<HighScore>)
    );
  }

}
